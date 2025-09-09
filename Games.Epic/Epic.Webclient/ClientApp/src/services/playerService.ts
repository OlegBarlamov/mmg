import { IPlayerInfo } from "./serverAPI";

export interface IPlayerService {
    get currentPlayerInfo(): IPlayerInfo
    setCurrentPlayerInfo(playerInfo: IPlayerInfo): void
}

export class PlayerService implements IPlayerService {
    private _currentPlayerInfo: IPlayerInfo | null = null

    get currentPlayerInfo(): IPlayerInfo {
        return this._currentPlayerInfo!
    }

    setCurrentPlayerInfo(playerInfo: IPlayerInfo): void {
        this._currentPlayerInfo = playerInfo
    }
}