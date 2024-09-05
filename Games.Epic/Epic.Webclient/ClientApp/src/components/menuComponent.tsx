import React, {PureComponent} from "react";
import {IBattleDefinition} from "../battle/battleDefinition";
import {IServiceLocator} from "../services/serviceLocator";

export interface IMenuComponentProps {
    serviceLocator: IServiceLocator
    onBattleSelected(definition: IBattleDefinition): void
}

interface IMenuComponentState {
    availableBattles: IBattleDefinition[] | null 
}

export class MenuComponent extends PureComponent<IMenuComponentProps, IMenuComponentState> {
    constructor(props: IMenuComponentProps) {
        super(props)
        
        this.state = {
            availableBattles: null
        }
    }
    
    async componentDidMount() {
        const battlesProvider = this.props.serviceLocator.battlesProvider()
        const battles = await battlesProvider.fetchBattles()
        this.setState({availableBattles: battles})
    }
    
    render() {
        return (
            <div>
                <h1>Select a Battle</h1>
                {
                    this.state.availableBattles ? (
                    this.state.availableBattles.map((battle, index) => (
                            <button key={index} onClick={() => this.props.onBattleSelected(battle)}>
                                Size: {battle.mapWidth}x{battle.mapHeight}
                            </button>
                        ))
                    ) : (<div>Loading...</div>)
                }
            </div>
        )
    }
}