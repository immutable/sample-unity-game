<div align="center">
  <p align="center">
    <a  href="https://docs.x.immutable.com/docs">
      <img src="https://cdn.dribbble.com/users/1299339/screenshots/7133657/media/837237d447d36581ebd59ec36d30daea.gif" width="280"/>
    </a>
  </p>
</div>

---

# Sample Passport Unity Game

As an example of using the [Immutable Unity SDK](https://github.com/immutable/unity-immutable-sdk), we'll use a Unity game called “Immutable Runner”, a customised version based on the [Unity Runner game template](https://unity.com/features/build-a-runner-game). In this game, the player takes on the role of an adventurous fox who must collect Immutable coins and food while avoiding obstacles like trees and bears to reach the finish line.

What makes this game unique is that the Fox, Skins and Immutable Runner coins will be represented as NFTs (ERC721). This means that players can own these digital assets. Moreover, players can use the Immutable Runner coins they have collected to create or [craft](https://docs.immutable.com/docs/zkevm/products/minting/crafting/#what-is-crafting) new skins for their fox. These skins will also be represented as NFTs, further enriching the player's ownership experience.

## Supported Platforms

* Windows
* Android
* iOS
* macOS

## Pre-requisite

Some files are stored on Git Large File Storage, so you must download and install git-lfs from [here](https://git-lfs.github.com/).

## How to run the game

### Prerequisites
> [!IMPORTANT]  
> This game is set up to work on the Testnet/Sandbox environment only

1. Register the game by following the [Immutable X](https://docs.immutable.com/docs/x/sdks/unity#registering-your-game) or  [Immutable zkEVM](https://docs.immutable.com/docs/zkEVM/sdks/unity#registering-your-game) guide
    1. Make sure to set the [Redirect URLs](https://docs.immutable.com/docs/x/sdks/unity#creating-an-oauth20-native-client) field to `imxsample://callback`
    1. Make sure to set the [Logout URLs](https://docs.immutable.com/docs/x/sdks/unity#creating-an-oauth20-native-client) field to `imxsample://callback/logout`
2. Open `Assets/Runner/Scripts/Config.cs` and set the client ID

### Prerequisites for Windows and macOS only

To support minting and crafting in the game, three NFT collections need to be present:
1. **Immutable Runner Token (IMR)** for the Immutable coins you see in the game
2. **Immutabler Runner Character (IMRC)** for the Fox Runner character you use to play the game
3. **Immutable Runner Skin (IMRS)** for any skin you manage to unlock and claim

#### Immutable zkEVM

1. [Set up your development wallet](https://docs.immutable.com/docs/zkEVM/guides/wallet)
2. [Obtain Test-$IMX](https://docs.immutable.com/docs/zkEVM/guides/faucet) so you can send transactions (specifically to mint assets in this game) and deploy smart contracts on zkEVM Testnet
3. Follow [this](https://github.com/immutable/sample-passport-unity-game/blob/main/contracts/README.md) to deploy the [three zkEVM contracts](https://github.com/immutable/sample-passport-unity-game/tree/main/contracts/contracts) the game requires
4. Rename `server/.env.example` to `.env`:
    1. Update `PRIVATE_KEY` with your admin wallet's private key
    2. Update `ZK_TOKEN_TOKEN_ADDRESS` to the contract address of Immutable Runner Token
    3. Update `ZK_CHARACTER_TOKEN_ADDRESS` to the contract address of Immutable Character Token
    4. Update `ZK_SKIN_TOKEN_ADDRESS` to the contract address of Immutable Skin Token
    5. Update `ALCHEMY_API_KEY` with your [Alchemy](https://www.alchemy.com/) Sepolia API key
5. Open `Assets/Runner/Scripts/Config.cs` 
    1. Update`ZK_TOKEN_TOKEN_ADDRESS` to the contract address of Immutable Runner Token
    2. Update`ZK_SKIN_TOKEN_ADDRESS` to the contract address of Immutable Skin Token
6. The game uses [zkEVM Send Transactions](https://docs.immutable.com/docs/zkEVM/sdks/unity#zkevm-send-transaction) to perform crafting, so you must [contact us](https://docs.immutable.com/docs/x/contact/) for pre-approval. You will need to provide us:
    1. The name of the game
    2. The environment i.e. Testnet
    3. The client ID
    4. The `ZK_TOKEN_TOKEN_ADDRESS` and the function signature of the [craftSkin](https://github.com/immutable/sample-passport-unity-game/blob/d003639beb8b6ae91dbb590a20349d4ba67e79b2/contracts/contracts/RunnerToken.sol#L57) method, i.e. `0x63daf310`

#### Immutable X

1. Follow [this](https://docs.immutable.com/docs/x/zero-to-hero-nft-minting) guide on how to create the three NFT collections
2. Rename `server/.env.example` to `.env`:
    1. Update `PRIVATE_KEY` with your wallet's private key 
    2. Update `TOKEN_TOKEN_ADDRESS` to the contract address of Immutable Runner Token
    3. Update `CHARACTER_TOKEN_ADDRESS` to the contract address of Immutable Character Token
    4. Update `SKIN_TOKEN_ADDRESS` to the contract address of Immutable Skin Token
    5. Update `ALCHEMY_API_KEY` with your [Alchemy](https://www.alchemy.com/) Sepolia API key
3. Open `Assets/Runner/Scripts/Config.cs` 
    1. Update`TOKEN_TOKEN_ADDRESS` to the contract address of Immutable Runner Token
    2. Update`SKIN_TOKEN_ADDRESS` to the contract address of Immutable Skin Token
4. The game uses [Immutable X Single and Bulk transfer](https://docs.immutable.com/docs/x/sdks/unity/#immutable-x-transfer) to perform crafting, so you must [contact us](https://docs.immutable.com/docs/x/contact/) for pre-approval. You will need to provide us:
    1. The name of the game
    2. The environment i.e. Sandbox
    3. The client ID
    2. The `TOKEN_TOKEN_ADDRESS` and the receiver's address `0x0000000000000000000000000000000000000000`

## Running the game on Windows and macOS

Run the server once pre-approved to use Immutable X Single/Bulk transfer and/or zkEVM Send Transactions for crafting.

The local server is used to mint and help craft assets.

1. Navigate to `server/`
2. Run `npm install`
3. Run `npm run build`
4. Run `npm run dev`
5. Switch to the platform you would like to run on in Build Settings
6. Start the game
    1. If you are running the game inside the Editor, open `Assets/Shared/Scenes/Boot.unity`

## Running the game on Android and iOS

Run the server once pre-approved to use Immutable X Single/Bulk transfer and/or zkEVM Send Transactions for crafting.

The local server is used to mint and help craft assets.

1. Grab your local IP address
    1. On Mac (Ethernet connection): Enter `ipconfig getifaddr en1` in your terminal
    2. On Mac (Wireless connection): Enter `ipconfig getifaddr en0` in your terminal
    3. On Windows: Enter `ipconfig` in command prompt -> grab the IPv4 Address
2. Open `Assets/Runner/Scripts/Config.cs` 
3. Update the [`SERVER_BASE_URL`](https://github.com/immutable/sample-passport-unity-game/blob/be18aa694d1216b37cee900bf0c0cfa8571ac92a/Assets/Shared/Scripts/Config.cs#L18C49-L18C70) to your local IP address including the scheme (`http://`) and the port number (`6060`), e.g. `http://192.168.0.123:6060`
4. Navigate to `server/`
5. Run `npm install`
6. Run `npm run dev`
7. Switch to the platform you would like to run on in Build Settings
8. Start the game
    1. If you are running the game inside the Editor, open `Assets/Shared/Scenes/Boot.unity`


## Switching between Immutable X and zkEVM
On the main menu screen, there is a small checkbox at the top right that toggles between Immutable X and zkEVM. If checked, zkEVM will be used.

This flag is also saved in the game's `SaveManager`.

## Minting
The [local server](https://github.com/immutable/sample-passport-unity-game/tree/main/server) is used to mint assets the gamer collects directly to their wallet. See endpoints `/mint/*` for Immutable X and `zkmint/*` for zkEVM.

## Crafting

### zkEVM
Crafting is done by calling the craft function in the smart contract. This is done by using zkEVM Send Transaction.

For example, to claim the cooler skin, the user needs to burn their current skin first:
* [Get the encoded data](https://github.com/immutable/sample-unity-game/blob/a1544a14f7f960dc5741f1e0264e82860397a858/Assets/Shared/Scripts/UI/UnlockedSkinScreen.cs#L126) required to call the RunnerSkin contract's [`craftSkin` function](https://github.com/immutable/sample-passport-unity-game/blob/d003639beb8b6ae91dbb590a20349d4ba67e79b2/contracts/contracts/RunnerSkin.sol#L52) by calling [`POST /zk/skin/craftskin/encodeddata`](https://github.com/immutable/sample-passport-unity-game/blob/ed06ce54c77d0e53837e494ae3acb04b4e98f7df/server/src/routes/posts.ts#L13)
* Use zkEVM Send Transaction to [call the `craftSkin` function](https://github.com/immutable/sample-unity-game/blob/a1544a14f7f960dc5741f1e0264e82860397a858/Assets/Shared/Scripts/UI/UnlockedSkinScreen.cs#L127) by passing the RunnerSkin contract address and the encoded data
  * Note that the craftSkin function does all the burning and minting

### Immutable X
Crafting is done by using the Immutable X Single and Bulk transfer functions.

Required asset(s) to craft a new asset is transferred to `0x0000000000000000000000000000000000000000` to burn them. Once that is successful, the new asset is minted to the gamer's wallet using the local server.

For example, to claim the first skin, the user needs to burn three of their tokens:
* [Burn three tokens](https://github.com/immutable/sample-unity-game/blob/a1544a14f7f960dc5741f1e0264e82860397a858/Assets/Shared/Scripts/UI/UnlockedSkinScreen.cs#L146) using Immutable X Bulk Transfer
* [Mint skin](https://github.com/immutable/sample-unity-game/blob/a1544a14f7f960dc5741f1e0264e82860397a858/Assets/Shared/Scripts/UI/UnlockedSkinScreen.cs#L152) using the [`POST /mint/skin`](https://github.com/immutable/sample-passport-unity-game/blob/ed06ce54c77d0e53837e494ae3acb04b4e98f7df/server/src/routes/posts.ts#L7)
