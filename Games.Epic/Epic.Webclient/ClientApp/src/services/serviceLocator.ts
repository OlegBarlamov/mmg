import {CanvasService, ICanvasService} from "./canvasService";
import {BattleMapsService, IBattleMapsService} from "./battleMapsService";

export interface IServiceLocator {
    canvasService() : ICanvasService
    battleMapService() : IBattleMapsService
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
}