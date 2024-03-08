// Copyright (c) Immutable Australia Pty Ltd 2018 - 2024
// SPDX-License-Identifier: MIT
pragma solidity 0.8.19;

import "@imtbl/contracts/contracts/token/erc721/preset/ImmutableERC721.sol";

// The skin you manage to unlock and claim in the game
contract RunnerSkin is ImmutableERC721 {
    uint256 private _currentTokenId = 0;

    constructor(
        address owner,
        string memory baseURI,
        string memory contractURI,
        address operatorAllowlist,
        address receiver,
        uint96 feeNumerator
    )
        ImmutableERC721(
            owner,
            "Immutable Runner Skin", // name
            "IMRS", // symbol
            baseURI,
            contractURI,
            operatorAllowlist,
            receiver,
            feeNumerator
        )
    {}
}