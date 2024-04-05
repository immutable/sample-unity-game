// Copyright (c) Immutable Australia Pty Ltd 2018 - 2024
// SPDX-License-Identifier: MIT
pragma solidity 0.8.19;

import "@openzeppelin/contracts/access/AccessControl.sol";
import "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import "@openzeppelin/contracts/token/ERC20/extensions/ERC20Burnable.sol";
import "@imtbl/contracts/contracts/access/MintingAccessControl.sol";
import "@imtbl/contracts/contracts/token/erc721/preset/ImmutableERC721.sol";

contract RunnerToken is ERC20, ERC20Burnable, MintingAccessControl {
    // A reference to the Immutable Runner Skin contract for the craftSkin function to use.
    // Note: Immutable Runner Skin contract simply extends ImmutableERC721, so we can set
    // the type to ImmutableERC721
    ImmutableERC721 private _skinContract;

    constructor(
        address skinContractAddr
    ) ERC20("Immutable Runner Token", "IMR") {
        // Grant the contract deployer the default admin role: it will be able
        // to grant and revoke any roles
        _setupRole(DEFAULT_ADMIN_ROLE, msg.sender);
        // Save the Immutable Runner Skin contract address
        _skinContract = ImmutableERC721(skinContractAddr);

        // Uncomment the line below to grant minter role to contract deployer
        // _grantRole(MINTER_ROLE, msg.sender);
    }

    // Mints the number of tokens specified
    function mint(address to, uint256 amount) external onlyRole(MINTER_ROLE) {
        _mint(to, amount);
    }

    // Burns three tokens and crafts a skin to the caller
    function craftSkin() external {
        uint256 numTokens = 3 * 10 ** decimals();

        // Burn caller's three tokens
        _burn(msg.sender, numTokens);

        // Mint one Immutable Runner Skin to the caller
        // Note: To mint a skin, the Immutable Runner Token contract must have the
        // Immutable Runner Skin contract minter role.
        _skinContract.mintByQuantity(msg.sender, 1);
    }
}
