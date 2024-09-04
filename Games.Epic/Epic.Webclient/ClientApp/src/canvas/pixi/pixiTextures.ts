import * as PIXI from "pixi.js";

const texturesCache = new Map<string, PIXI.Texture>();
export async function getTexture(imgSrc: string) : Promise<PIXI.Texture> {
    if (texturesCache.has(imgSrc)) {
        return texturesCache.get(imgSrc)!;
    }
    
    const texture = await PIXI.Assets.load(imgSrc);
    debugger
    if (texture)
        texturesCache.set(imgSrc, texture)
    
    return texture;
}