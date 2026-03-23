# WireGuard VPN Setup — VPS 1 (Infrastructure) → VPS 2 (Application)

## Overview

This document covers setting up a WireGuard VPN tunnel between two VPS machines so that application services on VPS 2 can securely connect to PostgreSQL, Redis, and RabbitMQ running on VPS 1.

```
VPS 2 (App)  ──── WireGuard Tunnel ────  VPS 1 (Infra)
10.13.13.2                               10.13.13.1
                                         ├── PostgreSQL :5432
                                         ├── Redis      :6379
                                         └── RabbitMQ   :5672
```

### Why this approach?
- PostgreSQL, Redis, RabbitMQ are bound to `127.0.0.1` on VPS 1 (for SSH tunnel access from local machine)
- WireGuard + iptables DNAT bridges the tunnel interface (`wg0`) to the loopback-bound services
- Services are never exposed to the public internet

---

## VPS 1 — Infrastructure Server

### 1. Cloud Init (first boot)

Include WireGuard in your cloud-init:

```yaml
#cloud-config
package_update: true
packages:
  - git
  - ufw
  - ca-certificates
  - curl
  - wireguard
  - wireguard-tools
runcmd:
  # Docker setup
  - install -m 0755 -d /etc/apt/keyrings
  - curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
  - chmod a+r /etc/apt/keyrings/docker.asc
  - |
    tee /etc/apt/sources.list.d/docker.sources <<EOF
    Types: deb
    URIs: https://download.docker.com/linux/ubuntu
    Suites: $(. /etc/os-release && echo "${UBUNTU_CODENAME:-$VERSION_CODENAME}")
    Components: stable
    Signed-By: /etc/apt/keyrings/docker.asc
    EOF
  - apt update
  - apt install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
  - systemctl enable docker
  - systemctl start docker
  # WireGuard
  - echo "net.ipv4.ip_forward=1" >> /etc/sysctl.conf
  - sysctl -p
  - mkdir -p /etc/wireguard
  - systemctl enable wg-quick@wg0
  # Firewall
  - ufw allow OpenSSH
  - ufw allow 80
  - ufw allow 443
  - ufw allow 51820/udp
  - ufw --force enable
  # Clone project
  - mkdir -p /root/hyperdatalab
  - git clone https://github.com/scilab-org/scilab-microservices.git /root/hyperdatalab/scilab-microservices
```

### 2. Generate WireGuard Keys and Configs

Run this script **on VPS 1** to generate all keys and both config files in one shot:

```bash
# Generate key pairs for both VPS
wg genkey | tee /etc/wireguard/vps1_private.key | wg pubkey > /etc/wireguard/vps1_public.key
wg genkey | tee /etc/wireguard/vps2_private.key | wg pubkey > /etc/wireguard/vps2_public.key

# Read into variables
VPS1_PRIVATE=$(cat /etc/wireguard/vps1_private.key)
VPS1_PUBLIC=$(cat /etc/wireguard/vps1_public.key)
VPS2_PRIVATE=$(cat /etc/wireguard/vps2_private.key)
VPS2_PUBLIC=$(cat /etc/wireguard/vps2_public.key)

# Generate VPS 1 wg0.conf
cat > /etc/wireguard/wg0.conf <<EOF
[Interface]
Address = 10.13.13.1/24
ListenPort = 51820
PrivateKey = ${VPS1_PRIVATE}

# Enable routing to loopback-bound services
PostUp = sysctl -w net.ipv4.conf.wg0.route_localnet=1
PostUp = sysctl -w net.ipv4.conf.all.route_localnet=1

# Allow WireGuard traffic through FORWARD chain
PostUp = iptables -I FORWARD 1 -i wg0 -j ACCEPT
PostUp = iptables -I FORWARD 1 -o wg0 -j ACCEPT

# DNAT: forward tunnel traffic to loopback-bound Docker services
PostUp = iptables -t nat -A PREROUTING -i wg0 -p tcp --dport 5432 -j DNAT --to-destination 127.0.0.1:5432
PostUp = iptables -t nat -A PREROUTING -i wg0 -p tcp --dport 6379 -j DNAT --to-destination 127.0.0.1:6379
PostUp = iptables -t nat -A PREROUTING -i wg0 -p tcp --dport 5672 -j DNAT --to-destination 127.0.0.1:5672
PostUp = iptables -t nat -A POSTROUTING -j MASQUERADE

# Cleanup on shutdown
PostDown = iptables -D FORWARD -i wg0 -j ACCEPT
PostDown = iptables -D FORWARD -o wg0 -j ACCEPT
PostDown = iptables -t nat -D PREROUTING -i wg0 -p tcp --dport 5432 -j DNAT --to-destination 127.0.0.1:5432
PostDown = iptables -t nat -D PREROUTING -i wg0 -p tcp --dport 6379 -j DNAT --to-destination 127.0.0.1:6379
PostDown = iptables -t nat -D PREROUTING -i wg0 -p tcp --dport 5672 -j DNAT --to-destination 127.0.0.1:5672
PostDown = iptables -t nat -D POSTROUTING -j MASQUERADE

[Peer]
# VPS 2
PublicKey = ${VPS2_PUBLIC}
AllowedIPs = 10.13.13.2/32
EOF

# Generate VPS 2 wg0.conf (copy this to VPS 2)
cat > /etc/wireguard/vps2_wg0.conf <<EOF
[Interface]
Address = 10.13.13.2/24
PrivateKey = ${VPS2_PRIVATE}

[Peer]
# VPS 1
PublicKey = ${VPS1_PUBLIC}
Endpoint = $(curl -4 -s ifconfig.me):51820
AllowedIPs = 10.13.13.1/32
PersistentKeepalive = 25
EOF

# Lock down permissions
chmod 600 /etc/wireguard/*.key
chmod 600 /etc/wireguard/*.conf

echo "=== VPS 1 config ===" && cat /etc/wireguard/wg0.conf
echo "=== VPS 2 config ===" && cat /etc/wireguard/vps2_wg0.conf
```

### 3. UFW — Allow WireGuard Services

```bash
# Allow inbound on wg0 to reach Docker services
sudo ufw allow in on wg0 to any port 5432
sudo ufw allow in on wg0 to any port 6379
sudo ufw allow in on wg0 to any port 5672
sudo ufw reload
```

> ⚠️ This step is required. UFW's INPUT policy is DROP by default — even after DNAT rewrites the destination, UFW will drop the packet without these rules.

### 4. Start WireGuard on VPS 1

```bash
sudo wg-quick up wg0

# Verify
sudo wg show
```

---

## VPS 2 — Application Server

### 1. Cloud Init (first boot)

Same cloud-init as VPS 1 with WireGuard packages included (see above). No need for Docker if this VPS only runs app services.

### 2. Copy VPS 2 Config from VPS 1

On VPS 1, print the generated config:
```bash
cat /etc/wireguard/vps2_wg0.conf
```

On VPS 2, paste it:
```bash
sudo nano /etc/wireguard/wg0.conf
```

> ⚠️ Make sure the `Endpoint` IP is VPS 1's **IPv4** public IP, not IPv6. If the script picked IPv6, replace it manually with the IPv4 address. You can get it with:
> ```bash
> curl -4 ifconfig.me   # run on VPS 1
> ```

### 3. Start WireGuard on VPS 2

```bash
sudo wg-quick up wg0

# Verify tunnel and handshake
sudo wg show
ping 10.13.13.1
```

Expected output from `wg show` on VPS 2:
```
interface: wg0
  public key: <VPS2_PUBLIC>
  private key: (hidden)
  listening port: <random>

peer: <VPS1_PUBLIC>
  endpoint: <VPS1_IPV4>:51820
  allowed ips: 10.13.13.1/32
  latest handshake: X seconds ago     ← must appear
  persistent keepalive: every 25 seconds
```

---

## Connection Strings on VPS 2

```env
# PostgreSQL
DATABASE_URL=postgresql://<user>:<password>@10.13.13.1:5432/<dbname>

# Redis
REDIS_ENDPOINT=10.13.13.1:6379
REDIS_PASSWORD=<password>

# RabbitMQ
RABBITMQ_HOST=10.13.13.1
RABBITMQ_PORT=5672
RABBITMQ_USER=<user>
RABBITMQ_PASS=<password>
```

---

## Verification Checklist

Run these from VPS 2 after both sides are up:

```bash
# 1. Tunnel is alive
ping 10.13.13.1

# 2. Service ports are reachable
nc -zv 10.13.13.1 5432    # PostgreSQL
nc -zv 10.13.13.1 6379    # Redis
nc -zv 10.13.13.1 5672    # RabbitMQ
```

All three `nc` commands should return `Connection to 10.13.13.1 XXXX port succeeded!`

---

## Auto-start on Boot

```bash
# On both VPS
sudo systemctl enable wg-quick@wg0
```

---

## Troubleshooting

### Ping hangs (no response)
```bash
# Check WireGuard is up on both sides
sudo wg show

# VPS 2: check endpoint is IPv4, not IPv6
cat /etc/wireguard/wg0.conf | grep Endpoint
# Must be: Endpoint = <IPv4>:51820, NOT [IPv6]:51820
```

### nc hangs (port unreachable)
```bash
# On VPS 1 — check packets arrive on tunnel
sudo tcpdump -i wg0 port 5432

# Check DNAT rules are active
sudo iptables -t nat -L PREROUTING -n -v | grep 5432

# Check route_localnet is enabled
cat /proc/sys/net/ipv4/conf/wg0/route_localnet   # must be 1
cat /proc/sys/net/ipv4/conf/all/route_localnet    # must be 1

# Check UFW allows inbound on wg0
sudo ufw status | grep 5432
```

### Handshake never appears in `wg show`
```bash
# On VPS 1 — check UDP 51820 is open
sudo ufw status | grep 51820

# On VPS 2 — check endpoint IP is correct
cat /etc/wireguard/wg0.conf | grep Endpoint
```

### DNS stops working after WireGuard starts (`communications error to 127.0.0.53`)

**Cause:** The broad `MASQUERADE` rule rewrites the source IP of every outgoing packet on the host — including DNS queries from systemd-resolved — which breaks local DNS resolution.

```ini
# This was the problematic rule (too broad)
PostUp = iptables -t nat -A POSTROUTING -j MASQUERADE
```

**Fix 1 — Scope the MASQUERADE rule** (already applied in `wg0.conf` above):
```ini
# Only masquerade WireGuard subnet traffic
PostUp = iptables -t nat -A POSTROUTING -s 10.13.13.0/24 -j MASQUERADE
```

**Fix 2 — Bypass broken systemd-resolved permanently:**
```bash
sudo rm /etc/resolv.conf
echo "nameserver 8.8.8.8" | sudo tee /etc/resolv.conf
echo "nameserver 1.1.1.1" | sudo tee -a /etc/resolv.conf

# Prevent cloud-init or systemd from overwriting it
sudo chattr +i /etc/resolv.conf
```

Verify:
```bash
nslookup github.com   # should resolve correctly now
```

---

### Restart WireGuard cleanly
```bash
sudo ip link delete wg0 2>/dev/null; sudo wg-quick up wg0
```

---

## SSH Tunnel — Local Machine to VPS 1

For local development, forward VPS 1 service ports to your local machine via SSH:

```bash
ssh -L 5432:127.0.0.1:5432 \
    -L 6379:127.0.0.1:6379 \
    -L 5672:127.0.0.1:5672 \
    -L 15672:127.0.0.1:15672 \
    root@<VPS1_PUBLIC_IP> -N
```

Then connect locally as if the services were running on your machine:

```bash
# PostgreSQL
psql -h 127.0.0.1 -p 5432 -U postgres

# Redis
redis-cli -h 127.0.0.1 -p 6379 -a <password>

# RabbitMQ management UI
open http://127.0.0.1:15672
```

To run the tunnel **silently in the background**:

```bash
ssh -L 5432:127.0.0.1:5432 \
    -L 6379:127.0.0.1:6379 \
    -L 5672:127.0.0.1:5672 \
    -L 15672:127.0.0.1:15672 \
    -o ServerAliveInterval=60 \
    -o ExitOnForwardFailure=yes \
    -fNT root@<VPS1_PUBLIC_IP>
```

- `-f` — run in background
- `-N` — no command, tunnel only
- `-T` — disable pseudo-terminal
- `ServerAliveInterval=60` — keeps the tunnel alive
- `ExitOnForwardFailure=yes` — exits cleanly if port forwarding fails

To kill the background tunnel:
```bash
pkill -f "ssh -L 5432"
```

---

## Network Architecture Summary

```
[Local Machine]
     │
     │ SSH tunnel (local port forwarding)
     │ ssh -L 5432:127.0.0.1:5432 root@VPS1
     ▼
[VPS 1 — Infrastructure]  10.13.13.1
  iptables DNAT (wg0 → 127.0.0.1)
  UFW allows inbound on wg0
     │
     │  Docker services bound to 127.0.0.1
     ├── PostgreSQL  :5432
     ├── Redis       :6379
     ├── RabbitMQ    :5672
     └── (Keycloak, MinIO, etc.)
     ▲
     │ WireGuard tunnel (UDP 51820)
     │
[VPS 2 — Application]  10.13.13.2
  Connects to 10.13.13.1:5432/6379/5672
  via encrypted WireGuard tunnel
```