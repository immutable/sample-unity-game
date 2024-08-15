"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = __importDefault(require("express"));
const cors_1 = __importDefault(require("cors"));
const http_1 = __importDefault(require("http"));
const foxImageUrl = 'https://rose-ministerial-termite-701.mypinata.cloud/ipfs/Qmd3oT99HypRHaPfiY6JWokxADR5TzR1stgonFy1rMZAUy';
const skinImageUrl = 'https://rose-ministerial-termite-701.mypinata.cloud/ipfs/QmNZeG8wkW3mFw4PrqEj34NPA88impcvemYjhAkJAM4YcK';
// Custom skin
const skinColours = ['Tropical Indigo', 'Cyclamen', 'Robin Egg Blue', 'Mint', 'Mindaro', 'Amaranth Pink'];
const skinImageUrls = [
    'QmQApmc7UcjAC4txHd2ufpbpadisYyo79QTY69X9QarcLV',
    'QmT1iSRnX8YkJj88Cbw6pfmCCQzxGs84UWSfbd471HPsj8',
    'QmRqKuUT3oHcg9ib7t4ePPomD7P1fmYv3Xu1Km4KTn4Ddj',
    'QmcC18L7DXz37GjihGQiLrH3kxgUwb29dXQfddEJjuLn1W',
    'Qmd6LMHBxAXau8ZWs9ydgxZtWLSjV89q9zb6awwscJZbQt',
    'QmXDfVp7jf11wiAPSEEpJDGKN1vYkFDKTTpJfPhjABRd77'
];
const skinSpeeds = ['Slow', 'Medium', 'Fast'];
const app = (0, express_1.default)();
app.use((0, cors_1.default)());
const router = express_1.default.Router();
router.get('/fox/:id', async (req, res) => {
    const json = {
        id: parseInt(req.params.id),
        name: `Fox #${req.params.id}`,
        image: foxImageUrl,
    };
    res.writeHead(200);
    res.end(JSON.stringify(json));
});
// Skins gamers can get by crafting only
router.get('/skin/celestial-blue/:id', async (req, res) => {
    const id = parseInt(req.params.id);
    const json = {
        id: parseInt(req.params.id),
        name: `Celestial Blue Skin #${req.params.id}`,
        image: skinImageUrl,
        attributes: [
            {
                "trait_type": "Colour",
                "value": "Celestial Blue"
            },
            {
                "trait_type": "Speed",
                "value": "Fast"
            }
        ]
    };
    res.writeHead(200);
    res.end(JSON.stringify(json));
});
// Skins gamers can buy from the marketplace
router.get('/skin/:id', async (req, res) => {
    const id = parseInt(req.params.id);
    const json = {
        id,
        name: `${skinColours[id % 6]} Skin #${req.params.id}`,
        image: `https://rose-ministerial-termite-701.mypinata.cloud/ipfs/${skinImageUrls[id % 6]}`,
        attributes: [
            {
                "trait_type": "Colour",
                "value": skinColours[id % 6] // For demo purposes, the colour is deterministic
            },
            {
                "trait_type": "Speed",
                "value": skinSpeeds[id % 3] // For demo purposes, the speed is deterministic
            }
        ]
    };
    res.writeHead(200);
    res.end(JSON.stringify(json));
});
app.use('/', router);
http_1.default.createServer(app).listen(3000, () => console.log('Listening on port 3000'));
