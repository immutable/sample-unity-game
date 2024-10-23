import * as dotenv from "dotenv";
import { getEnv } from "../libs/utils";

dotenv.config();

export default {
  alchemyApiKey: getEnv("ALCHEMY_API_KEY"),
  ethNetwork: getEnv("ETH_NETWORK"),
  privateKey: getEnv("PRIVATE_KEY"),
  // Mint tokens
  tokenTokenAddress: getEnv("TOKEN_TOKEN_ADDRESS"),
  zkTokenTokenAddress: getEnv("ZK_TOKEN_TOKEN_ADDRESS"),
  // Mint characters
  characterTokenAddress: getEnv("CHARACTER_TOKEN_ADDRESS"),
  zkCharacterTokenAddress: getEnv("ZK_CHARACTER_TOKEN_ADDRESS"),
  // Mint skins
  skinTokenAddress: getEnv("SKIN_TOKEN_ADDRESS"),
  zkSkinTokenAddress: getEnv("ZK_SKIN_TOKEN_ADDRESS"),
  // Fox Mint tokens
  foxTokenAddress: getEnv("FOX_TOKEN_CONTRACT_ADDRESS"),
  // Fox Mint Fox
  foxContractAddress: getEnv("FOX_CONTRACT_ADDRESS"),
  // In game trading
  inGameTradingTokenAddress: getEnv("IN_GAME_TRADING_TOKEN_CONTRACT_ADDRESS"),
  inGameTradingFoxAddress: getEnv("IN_GAME_TRADING_FOX_CONTRACT_ADDRESS"),
  inGameTradingSkinBlueAddress: getEnv(
    "IN_GAME_TRADING_SKIN_BLUE_CONTRACT_ADDRESS",
  ),
  inGameTradingSkinColourAddress: getEnv(
    "IN_GAME_TRADING_SKIN_COLOUR_CONTRACT_ADDRESS",
  ),
  inGameTradingPackAddress: getEnv(
    "IN_GAME_TRADING_PACK_COLOUR_CONTRACT_ADDRESS",
  ),
};
