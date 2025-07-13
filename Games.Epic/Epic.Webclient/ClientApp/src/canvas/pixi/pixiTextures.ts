import * as PIXI from "pixi.js";

const dummyTexture = getDummyTexture()
const texturesCache = new Map<string, PIXI.Texture>();
export async function getTexture(imgSrc: string) : Promise<PIXI.Texture> {
    if (texturesCache.has(imgSrc)) {
        return texturesCache.get(imgSrc)!
    }
    
    try {
        const texture = await PIXI.Assets.load(imgSrc)
        if (texture)
            texturesCache.set(imgSrc, texture)
        
        return texture
    } catch (e) {
        console.error(e)
        return dummyTexture
    }
}

function getDummyTexture(): PIXI.Texture {
    return PIXI.Texture.WHITE
}