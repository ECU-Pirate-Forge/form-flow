import React from "react";
import { QuestionDefinition } from "../types/QuestionDefinition";

interface Props {
    question: QuestionDefinition;
}

export const QuestionRenderer: React.FC<Props> = ({ question }) => {
    return (
        <div style={{ maxWidth: "400px", marginBottom: "1rem"}}>
            <label style={{ display: "block", fontWeight: 600, marginBottom: "4px"}}>
                {question.label}
                {question.required && (
                    <span style={{ color: "red", marginLeft: "4px"}}>*</span>
                )}
                </label>

            <input 
                type="text"
                placeholder={question.placeholder}
                required={question.required}
                defaultValue={question.defaultValue}
                style={{
                    padding: "8px",
                    borderRadius: "4px",
                    border: "1px solid #ccc",
                    width: "100%"
                }}
            />

            {question.helpText && (
                <small style={{ display: "block", marginTop: "4px", color: '#666'}}>
                    {question.helpText}
                </small>
            )}        
        </div>
    );
};