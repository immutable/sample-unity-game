import { ethers } from 'hardhat'; // eslint-disable-line import/no-extraneous-dependencies

async function main() {
  // Load the Immutable Runner Token contract and get the contract factory
  const tokenContractFactory = await ethers.getContractFactory('RunnerToken');

  // Deploy the contract to the zkEVM network
  const tokenContract = await tokenContractFactory.deploy(
    'YOUR_IMMUTABLE_RUNNER_SKIN_CONTRACT_ADDRESS', // Immutable Runner Skin contract address
  );

  const tokenContractAddress = await tokenContract.getAddress();
  console.log('Token contract deployed to:', tokenContractAddress);

  // Load the Immutable Runner Pack contract and get the contract factory
  const packContractFactory = await ethers.getContractFactory('RunnerPack');

  // Deploy the contract to the zkEVM network
  const packContract = await packContractFactory.deploy(
      tokenContractAddress, // Immutable Runner Token contract address
    '0x5F5EBa8133f68ea22D712b0926e2803E78D89221', // Immutable Operator Allowlist
  );

  console.log('Pack contract deployed to:', await packContract.getAddress());
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });