// Copyright (c) Immutable Australia Pty Ltd 2018 - 2024
// SPDX-License-Identifier: MIT
pragma solidity 0.8.19;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import "@imtbl/contracts/contracts/token/erc1155/preset/ImmutableERC1155.sol";

contract RunnerPack is ImmutableERC1155 {
    // A reference to the Immutable Runner Token contract for the buyPack function to use.
    ERC20 private _tokenContract;

    uint256 public constant GALACTIC_SHIELD = 1;
    uint256 public constant CLEAR_SKIES = 2;

    constructor(
        address tokenContractAddr,
        address operatorAllowlist
    )
        ImmutableERC1155(
            msg.sender,
            "Immutabler Runner Pack",
            "https://immutable-runner-assets.onrender.com/pack/{id}.json",
            "https://cyan-electric-peafowl-878.mypinata.cloud/ipfs/QmaYDfitA8BD9PHkSmr4bYZsSA8j53E764WaRKBqP19TPt",
            operatorAllowlist,
            address(0xEED04A543eb26cB79Fc41548990e105C08B16464),
            5
        )
    {
        // Grant the contract deployer the default admin role: it will be able
        // to grant and revoke any roles
        _setupRole(DEFAULT_ADMIN_ROLE, msg.sender);
        // Save the Immutable Runner Token contract address
        _tokenContract = ERC20(tokenContractAddr);

        // Grant minter role to contract deployer
        _grantRole(MINTER_ROLE, msg.sender);
    }

    // Galactic Shield Pack - 5 Galactic Shields
    function buyGalacticShieldPack() external {
        uint256 numTokens = 10 * (10 ** 18); // Costs 10 IMRs

        // Transfer the required amount of ERC20 tokens directly from sender to the contract
        bool success = _tokenContract.transferFrom(
            msg.sender,
            address(this),
            numTokens
        );
        require(success, "IMR transfer failed");

        uint256[] memory ids = new uint256[](1);
        uint256[] memory amounts = new uint256[](1);

        ids[0] = GALACTIC_SHIELD;
        amounts[0] = 5;

        // Call the batch minting function
        _mintBatch(msg.sender, ids, amounts, ""); // Mint the items to the caller
    }

    // Clear Skies Pack - 5 Clear Skies
    function buyClearSkiesPack() external {
        uint256 numTokens = 8 * (10 ** 18); // Costs 8 IMRs

        // Transfer the required amount of ERC20 tokens directly from sender to the contract
        bool success = _tokenContract.transferFrom(
            msg.sender,
            address(this),
            numTokens
        );
        require(success, "IMR transfer failed");

        uint256[] memory ids = new uint256[](1);
        uint256[] memory amounts = new uint256[](1);

        ids[0] = CLEAR_SKIES;
        amounts[0] = 5;

        // Call the batch minting function
        _mintBatch(msg.sender, ids, amounts, ""); // Mint the items to the caller
    }

    // Navigator’s Combo Pack - 3 Galactic Shields and 3 Clear Skies
    function buyNavigatorsComboPack() external {
        uint256 numTokens = 9 * (10 ** 18); // Costs 9 IMRs

        // Transfer the required amount of ERC20 tokens directly from sender to the contract
        bool success = _tokenContract.transferFrom(
            msg.sender,
            address(this),
            numTokens
        );
        require(success, "IMR transfer failed");

        uint256[] memory ids = new uint256[](2);
        uint256[] memory amounts = new uint256[](2);

        ids[0] = GALACTIC_SHIELD;
        amounts[0] = 3;

        ids[1] = CLEAR_SKIES;
        amounts[1] = 3;

        // Call the batch minting function
        _mintBatch(msg.sender, ids, amounts, ""); // Mint the items to the caller
    }
}
