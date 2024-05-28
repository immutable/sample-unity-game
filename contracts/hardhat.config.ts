/* eslint-disable import/no-extraneous-dependencies */
import { HardhatUserConfig } from 'hardhat/config';
import '@nomicfoundation/hardhat-toolbox';

import * as dotenv from 'dotenv';

dotenv.config();

const config: HardhatUserConfig = {
  solidity: {
    version: '0.8.19',
    settings: {
      optimizer: {
        enabled: true,
        runs: 200,
      },
    },
  },
  networks: {
    immutableZkevmTestnet: {
      url: 'https://rpc.testnet.immutable.com',
      accounts: process.env.PRIVATE_KEY ? [process.env.PRIVATE_KEY] : [],
    },
  },
  etherscan: {
    apiKey: process.env.ETHERSCAN_API_KEY,
    customChains: [
      {
        network: 'immutableZkevmTestnet',
        chainId: 13473,
        urls: {
          apiURL: 'https://explorer.testnet.immutable.com/api',
          browserURL: 'https://explorer.testnet.immutable.com',
        },
      },
    ],
  },
  sourcify: {
    // Disabled by default
    // Doesn't need an API key
    enabled: true,
  },
};

export default config;