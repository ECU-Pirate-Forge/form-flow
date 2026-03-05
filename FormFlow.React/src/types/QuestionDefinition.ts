export interface QuestionDefinition {
    id: string;
    label: string;
    type: string;
    placeholder?: string;
    required?: boolean;
    defaultValue?: string | number;
    helpText?: string;
}