import React from "react";
import { IResourceInfo } from "../services/serverAPI";
import "./resourcesView.css";

export interface IResourcesViewProps {
    resources: IResourceInfo[] | null
}

export const ResourcesView: React.FC<IResourcesViewProps> = ({ resources }) => {
    if (!resources || resources.length === 0) {
        return null;
    }

    return (
        <div className="resources-section">
            <div className="resources-container">
                {resources.map((resource) => (
                    <div 
                        key={resource.id} 
                        className="resource-item"
                        title={resource.name}
                    >
                        <img 
                            src={resource.iconUrl} 
                            alt={resource.name}
                            className="resource-icon"
                        />
                        <span className="resource-amount">{resource.amount}</span>
                    </div>
                ))}
            </div>
        </div>
    );
}; 