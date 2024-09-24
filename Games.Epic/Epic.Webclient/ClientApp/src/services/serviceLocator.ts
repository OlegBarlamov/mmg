import {CanvasService, ICanvasService} from "./canvasService";
import {BattleMapsService, IBattleMapsService} from "./battleMapsService";
import {IBattlesProvider, ServerBattlesProvider} from "./battlesProvider";
import {IBattleLoader, ServerBattleLoader} from "./battleLoader";
import {BattlesService, IBattlesService} from "./battlesService";
import {IServerAPI} from "./serverAPI";
import {FakeServerAPI} from "../server/FakeServerAPI";

export interface IServiceLocator {
    canvasService() : ICanvasService
    battleMapService() : IBattleMapsService
    battlesProvider() : IBattlesProvider
    battleLoader(): IBattleLoader
    battlesService(): IBattlesService
    serverAPI(): IServerAPI
}

export class ExplicitServiceLocator implements IServiceLocator {
    private singletons = new Map<string, object>()
    public getSingletoneService<T extends object>(serviceName: string, factory: () => T) : T  {
        if (this.singletons.has(serviceName)) {
            return this.singletons.get(serviceName) as T;
        }

        const instance = factory()
        this.singletons.set(serviceName, instance)
        return instance
    }
    
    canvasService(): ICanvasService {
        return this.getSingletoneService("CanvasService", () => new CanvasService())
    }

    battleMapService(): IBattleMapsService {
        return this.getSingletoneService("BattleMapService", () => new BattleMapsService(
            this.canvasService(),
        ))
    }

    battlesProvider(): IBattlesProvider {
        return this.getSingletoneService("BattlesProvider", () => new ServerBattlesProvider(
            this.serverAPI()
        ))
    }
    
    battleLoader(): IBattleLoader {
        return this.getSingletoneService("BattleLoader", () => new ServerBattleLoader(
            this.serverAPI()
        ))
    }

    battlesService(): IBattlesService {
        return this.getSingletoneService("BattlesService", () => new BattlesService(
            this.battleMapService(),
            this.serverAPI()
        ))
    }

    serverAPI(): IServerAPI {
        return this.getSingletoneService("ServerAPI", () => new FakeServerAPI())
    }
}