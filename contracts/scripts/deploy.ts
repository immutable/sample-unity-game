import { ethers } from 'hardhat'; // eslint-disable-line import/no-extraneous-dependencies

async function main() {
  // Load the Immutable Runner Token contract and get the contract factory
  // const tokenContractFactory = await ethers.getContractFactory('RunnerToken');

  // Deploy the contract to the zkEVM network
  // const tokenContract = await tokenContractFactory.deploy(
  //   '0x3318a6a5135b8daa9997b34ab140d34e9443f48d', // Immutable Runner Skin contract address
  // );

  // console.log('Contract deployed to:', await tokenContract.getAddress());

  // Load the Immutable Runner Pack contract and get the contract factory
  const packContractFactory = await ethers.getContractFactory('RunnerPack');

  // Deploy the contract to the zkEVM network
  const packContract = await packContractFactory.deploy(
    '0x5838241BCABB660f999264bCda571Fc59931d17e', // Immutable Runner Token contract address
    '0x5F5EBa8133f68ea22D712b0926e2803E78D89221', // Immutable Operator Allowlist
  );

  console.log('Contract deployed to:', await packContract.getAddress());
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });