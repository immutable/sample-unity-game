import { ethers } from 'hardhat'; // eslint-disable-line import/no-extraneous-dependencies

async function main() {
  // Load the Immutable Runner Tokencontract and get the contract factory
  // const tokenContractFactory = await ethers.getContractFactory('RunnerToken');

  // Deploy the contract to the zkEVM network
  // const tokenContract = await tokenContractFactory.deploy(
  //   'YOUR_IMMUTABLE_RUNNER_SKIN_CONTRACT_ADDRESS', // Immutable Runner Skin contract address
  // );

  // console.log('Contract deployed to:', await tokenContract.getAddress());

  const packContractFactory = await ethers.getContractFactory('RunnerPack');

  // Deploy the contract to the zkEVM network
  const packContract = await packContractFactory.deploy(
    '0xb237501b35dfdcad274299236a141425469ab9ba', // Immutable Runner Token contract address
    '0x6b969FD89dE634d8DE3271EbE97734FEFfcd58eE' // Immutable Operator Allowlist
  );

  console.log('Contract deployed to:', await packContract.getAddress());
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });