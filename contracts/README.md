## How to deploy the contract
1. Rename `contracts/.env.example` to `.env`, and set all parameters with the appropriate data for the environment these Sample game contracts will be deployed.
2. Run `yarn install`
3. Replace `YOUR_IMMUTABLE_RUNNER_SKIN_CONTRACT_ADDRESS` in `contracts/RunnerToken.sol` with your Immutable Runner Skin contract address
3. Run `yarn compile`
4. Run `yarn deploy`
