# YARN Blockchain

A lightweight, production-ready Proof-of-Work blockchain implementation in C#.
Supports peer-to-peer networking over HTTP, persistent storage, wallet key generation, and transaction signing.

---

## 🌎 Overview
**YARN** is a decentralized blockchain network where nodes validate transactions through Proof-of-Work (PoW). Each participant runs a node that:
- Maintains the blockchain (stored as `chain.json`)
- Mines new blocks (earns YARN coins)
- Broadcasts and synchronizes with other peers

This implementation is designed to be simple, yet robust, and can be extended or modified.

---

## 👷 Who Is This For?
- Developers who want to study blockchain internals
- Educators or learners who want a real working codebase
- Curious users who want to try running or mining their own coin

---

## ⚡ Features
- Proof-of-Work mining
- JSON-based blockchain persistence
- ECDSA-secured wallet system
- HTTP-based peer-to-peer protocol
- Easy command-line interface

---

## 📁 Project Structure
```
YarnBlockchain/
├── Program.cs              # Entry point with CLI loop
├── Blockchain.cs           # Blockchain logic
├── Wallet.cs               # Wallet with key gen and address
├── Models/
│   ├── Transaction.cs      # Transaction format
│   ├── Block.cs            # Block structure and mining
│   └── P2PNode.cs          # Peer discovery and HTTP networking
├── Yarn.Blockchain.csproj # Project file
```

---

## 🚀 Quick Start (for Beginners)

### 1. ✅ Install Requirements
- [.NET 9 SDK or newer](https://dotnet.microsoft.com/download)
- Visual Studio Code (optional)

### 2. 📂 Extract and Open
- Unzip the project folder
- Open in VS Code or terminal

### 3. ▶️ Run the App
```bash
cd YarnBlockchain

dotnet run
```

### 4. ⚖️ Use the Console
Once the program starts, you can type:
```
balance                # See how much YARN you own
send <addr> <amt>      # Send YARN to another address
mine                   # Mine a block and earn a reward
```

Your wallet will be auto-generated and saved to `wallet.pem`.

### 5. 🚂 Run Multiple Peers (Simulated Network)
Open new terminals and run:
```bash
dotnet run -- 127.0.0.1:52345
```
This will sync the new node with the original.

---

## ⚛️ How It Works

### 🔍 1. Blockchain
- Each block contains a list of verified transactions.
- Blocks are linked by hashes (like DNA chains).
- The chain starts with a genesis block.

### ⚒️ 2. Proof-of-Work
- Mining requires solving a cryptographic puzzle.
- This is controlled by `Difficulty = 3` (number of leading zeros in hash).
- Successful miners are rewarded with 1 YARN coin.

### 👥 3. Peer-to-Peer
- Nodes talk over HTTP on port `52345`
- Each node adds a peer via command-line argument
- Example:
```bash
dotnet run -- 192.168.1.25:52345
```

### 📅 4. Persistence
- Blockchain data is saved to `chain.json`
- Wallet is stored as `wallet.pem` (contains private key)

### 💼 5. Security
- Transactions are signed using ECDSA with `nistP256` curve
- Each wallet generates a private/public key pair
- Only valid, signed transactions are added to the chain

---

## 📌 Advanced Notes
- You can change the default port in `Program.cs`
- All communication is local HTTP, not encrypted — use VPN or TLS for secure usage
- The block reward and difficulty are adjustable in `Blockchain.cs`
- The wallet address is the SHA256 hash of your public key

---

## 🚫 Limitations
- Not suitable for production without TLS
- No mempool / UTXO set
- Chain resolution is not yet implemented (longest chain wins)

---

## ✉️ Future Work (Suggestions)
- Encrypted peer communication (TLS)
- Quantum-safe hashing (e.g. SHA3 or post-quantum algos)
- Persistent peer list
- GUI wallet and explorer

---

## 🌟 Credits
Designed and developed for educational and experimental purposes.

> Happy mining with YARN! 🧶

