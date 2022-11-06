import * as PIXI from "pixi.js";

export class StaticTextures {
    private static _kingTexture: PIXI.Texture | undefined
    public static get kingTexture(): PIXI.Texture {
        if (!StaticTextures._kingTexture) {
            StaticTextures._kingTexture =  PIXI.Texture.from('king_hearts.jpeg')
        }
        return this._kingTexture!
    } 
} 