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
const app = (0, express_1.default)();
app.use((0, cors_1.default)());
const router = express_1.default.Router();
router.get('/fox/:id', async (req, res) => {
    const json = {
        id: parseInt(req.params.id),
        name: `Fox #${req.params.id}`,
        image: foxImageUrl
    };
    res.writeHead(200);
    res.end(JSON.stringify(json));
});
router.get('/skin/:id', async (req, res) => {
    const json = {
        id: parseInt(req.params.id),
        name: `Skin #${req.params.id}`,
        image: skinImageUrl
    };
    res.writeHead(200);
    res.end(JSON.stringify(json));
});
app.use('/', router);
http_1.default.createServer(app).listen(3000, () => console.log(`Listening on port 3000`));
