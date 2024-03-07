import { ethers } from 'hardhat'; // eslint-disable-line import/no-extraneous-dependencies

async function main() {
  // Load the Immutable Runner Tokencontract and get the contract factory
  const contractFactory = await ethers.getContractFactory('RunnerToken');

  // Deploy the contract to the zkEVM network
  const contract = await contractFactory.deploy(
    'YOUR_IMMUTABLE_RUNNER_SKIN_CONTRACT_ADDRESS', // Immutable Runner Skin contract address
  );

  console.log('Contract deployed to:', await contract.getAddress());
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
