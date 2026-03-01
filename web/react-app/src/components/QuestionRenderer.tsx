import React from "react";
import { QuestionDefinition } from "../types/QuestionDefinition";

interface Props {
    question: QuestionDefinition;
}

export const QuestionRenderer: React.FC<Props> = ({ question }) => {
    return (
        <div>
            <label>{question.label}</label>

            <input 
                type="text"
                placeholder={question.placeholder}
                required={question.required}
                defaultValue={question.defaultValue}
            />

            {question.helpText && (
                <small style={{ display: "block", marginTop: "4px"}}>
                    {question.helpText}
                </small>
            )}        
        </div>
    );
};