import React from "react";
import { QuestionDefinition } from "../types/QuestionDefinition";

interface Props {
    question: QuestionDefinition;
}

export const QuestionRenderer: React.FC<Props> = ({ question }) => {
    const inputId = `question-${question.id}-${question.key}`;
    const helpTextId = question.helpText ? `${inputId}-help` : undefined;

    return (
        <div style={{ 
            maxWidth: "400px", 
            marginBottom: "1rem",
            padding: "16px",
          }}>
            <label htmlFor={inputId} style={{ display: "block", fontWeight: 600, marginBottom: "4px"}}>
                {question.label}
                {question.required && (
                    <span style={{ color: "red", marginLeft: "4px"}}>*</span>
                )}
                </label>

            <input 
                id={inputId}
                type="text"
                placeholder={question.placeholder}
                required={question.required}
                defaultValue={question.defaultValue ?? undefined}
                aria-describedby={helpTextId}
                style={{
                    padding: "8px",
                    borderRadius: "4px",
                    border: "1px solid #ccc",
                    width: "100%"
                }}
            />

            {question.helpText && (
                <small id={helpTextId} style={{ display: "block", marginTop: "4px", color: '#666'}}>
                    {question.helpText}
                </small>
            )}        
        </div>
    );
};