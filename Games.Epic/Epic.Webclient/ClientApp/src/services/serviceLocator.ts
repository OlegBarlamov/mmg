import {CanvasService, ICanvasService} from "./canvasService";
import {BattleMapsService, IBattleMapsService} from "./battleMapsService";
import {FakeBattlesProvider, IBattlesProvider} from "./battlesProvider";
import {FakeBattleLoader, IBattleLoader} from "./battleLoader";
import {BattlesService, IBattlesService} from "./battlesService";

export interface IServiceLocator {
    canvasService() : ICanvasService
    battleMapService() : IBattleMapsService
    battlesProvider() : IBattlesProvider
    battleLoader(): IBattleLoader
    battlesService(): IBattlesService
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
        return this.getSingletoneService("BattlesProvider", () => new FakeBattlesProvider())
    }
    
    battleLoader(): IBattleLoader {
        return this.getSingletoneService("BattleLoader", () => new FakeBattleLoader(
            this.battleMapService()
        ))
    }

    battlesService(): IBattlesService {
        return this.getSingletoneService("BattlesService", () => new BattlesService(
            this.battleMapService()
        ))
    }
}