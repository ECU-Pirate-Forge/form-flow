
## **Validation Engine Overview**

* The backend now includes a `QuestionValidationEngine` that evaluates responses against rule lists.
* Rules are provided as JSON (same structure used by the UI).
* Supported rule types: `MinLength`, `MaxLength`, `MinValue`, `MaxValue`, `Range`.
* The engine returns:
  * `true` if all rules pass
  * `false` plus a list of error messages if any rule fails
* This engine is used by `QuestionValidator` and will be used by future form submission endpoints.
