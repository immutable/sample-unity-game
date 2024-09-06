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
  // For Mint Skin
  foxSkinAddress: getEnv("FOX_SKIN_CONTRACT_ADDRESS"),
  // For Mint Colour Skin
  foxColourSkinAddress: getEnv("FOX_COLOUR_SKIN_CONTRACT_ADDRESS"),
};
