import React, {PureComponent} from "react";
import {IBattleDefinition} from "../battle/IBattleDefinition";
import {IServiceLocator} from "../services/serviceLocator";

export interface IMenuComponentProps {
    serviceLocator: IServiceLocator
    onBattleSelected(definition: IBattleDefinition): void
}

interface IMenuComponentState {
    availableBattles: IBattleDefinition[] | null 
    isLoading: boolean
}

export class MenuComponent extends PureComponent<IMenuComponentProps, IMenuComponentState> {
    constructor(props: IMenuComponentProps) {
        super(props)
        
        this.state = {
            availableBattles: null,
            isLoading: true
        }
    }
    
    async componentDidMount() {
        await this.fetchBattles()
    }
    
    // Expose this method so parent can call it
    public async refreshBattles() {
        await this.fetchBattles()
    }
    
    private async fetchBattles() {
        this.setState({ isLoading: true })
        try {
            const battlesProvider = this.props.serviceLocator.battlesProvider()
            const battles = await battlesProvider.fetchBattles()
            this.setState({availableBattles: battles, isLoading: false})
        } catch (error) {
            this.setState({ isLoading: false })
            console.error('Failed to fetch battles:', error)
        }
    }
    
    render() {
        return (
            <div>
                <h1>Select a Battle</h1>
                {
                    this.state.isLoading ? (
                        <div>Loading...</div>
                    ) : this.state.availableBattles ? (
                        this.state.availableBattles.map((battle, index) => (
                            <button key={index} onClick={() => this.props.onBattleSelected(battle)}>
                                Size: {battle.width}x{battle.height}
                            </button>
                        ))
                    ) : (
                        <div>No battles available</div>
                    )
                }
            </div>
        )
    }
}