import { ethers } from 'hardhat'; // eslint-disable-line import/no-extraneous-dependencies

async function main() {
  // Load the Immutable Runner Tokencontract and get the contract factory
  // const tokenContractFactory = await ethers.getContractFactory('RunnerToken');

  // // Deploy the contract to the zkEVM network
  // const tokenContract = await tokenContractFactory.deploy(
  //   'YOUR_IMMUTABLE_RUNNER_SKIN_CONTRACT_ADDRESS', // Immutable Runner Skin contract address
  // );

  // console.log('Contract deployed to:', await tokenContract.getAddress());

  const packContractFactory = await ethers.getContractFactory('RunnerPack');

  // Deploy the contract to the zkEVM network
  const packContract = await packContractFactory.deploy(
    '0x328766302e7617d0de5901f8da139dca49f3ec75', // Immutable Runner Token contract address
    '0x5A3461514af018c19A6F887d14840B05fED4c5b8' // Immutable Operator Allowlist
  );

  console.log('Contract deployed to:', await packContract.getAddress());
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });