// Copyright (c) Immutable Australia Pty Ltd 2018 - 2024
// SPDX-License-Identifier: MIT
pragma solidity 0.8.19;

import "@openzeppelin/contracts/access/AccessControl.sol";
import "@imtbl/contracts/contracts/token/erc20/ImmutableERC20.sol";
import "@imtbl/contracts/contracts/token/erc721/preset/ImmutableERC721.sol";

contract RunnerToken is ImmutableERC20, AccessControl {
    // Create a new role identifier for the minter role
    bytes32 public constant MINTER_ROLE = keccak256("MINTER_ROLE");
    // Create a new role identifier for the burner role
    bytes32 public constant BURNER_ROLE = keccak256("BURNER_ROLE");

    // A reference to the Immutable Runner Skin contract for the craftSkin function to use.
    // Note: Immutable Runner Skin contract simply extends ImmutableERC721, so we can set
    // the type to ImmutableERC721
    ImmutableERC721 private _skinContract;

    constructor(
        address skinContractAddr
    ) ImmutableERC20("Immutable Runner Token", "IMR") {
        // Grant the contract deployer the default admin role: it will be able
        // to grant and revoke any roles
        _setupRole(DEFAULT_ADMIN_ROLE, msg.sender);
        // Save the Immutable Runner Skin contract address
        _skinContract = ImmutableERC721(skinContractAddr);

        // Uncomment the line below to grant minter role to contract deployer
        // _grantRole(MINTER_ROLE, msg.sender);
        // Uncomment the line below to grant burner role to contract deployer
        // _grantRole(BURNER_ROLE, msg.sender);
    }

    // Allows admin to grant specified address the minter role
    function grantMinterRole(address to) public onlyRole(DEFAULT_ADMIN_ROLE) {
        _grantRole(MINTER_ROLE, to);
    }

    // Allows admin to grant specified address the burner role
    function grantBurnerRole(address to) public onlyRole(DEFAULT_ADMIN_ROLE) {
        _grantRole(BURNER_ROLE, to);
    }

    // Mints the number of tokens specified
    function mint(address to, uint256 amount) external onlyRole(MINTER_ROLE) {
        _mint(to, amount);
    }

    // Burns the number of tokens specified
    function burn(address from, uint256 amount) external onlyRole(BURNER_ROLE) {
        _burn(from, amount);
    }

    // Burns three tokens and crafts a skin to the caller
    function craftSkin() external {
        uint256 numTokens = 3 * 10 ** decimals();
        require(
            balanceOf(msg.sender) >= numTokens,
            "craftSkin: Caller does not have enough tokens"
        );

        // Burn caller's three tokens
        _burn(msg.sender, numTokens);

        // Mint one Immutable Runner Skin to the caller
        // Note: To mint a skin, the Immutable Runner Token contract must have the
        // Immutable Runner Skin contract minter role.
        _skinContract.mintByQuantity(msg.sender, 1);
    }
}
