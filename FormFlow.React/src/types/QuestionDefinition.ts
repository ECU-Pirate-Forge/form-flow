import { Option } from "./Option";
import { VisibleIf } from "./VisibleIf";

export interface QuestionDefinition {
    id: string;
    key: string;
    label: string;
    type: string;

    required?: boolean;
    placeholder?: string;
    defaultValue?: string | number | null;

    options?: Option[];
    visibleIf?: VisibleIf;

    validationConfigs?: string | null;

    helpText?: string;
}