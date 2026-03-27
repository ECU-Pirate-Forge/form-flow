import React from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";
import { QuestionRenderer } from "../../FormFlow.React/src/components/QuestionRenderer";
import { QuestionDefinition } from "../../FormFlow.React/src/types/QuestionDefinition";

const makeQuestion = (overrides: Partial<QuestionDefinition>): QuestionDefinition => ({
  id: "00000000-0000-0000-0000-000000000000",
  key: "default_key",
  label: "Default Label",
  type: "text",
  required: false,
  defaultValue: "",
  ...overrides,
});

describe("QuestionRenderer", () => {
  test("renders correctly in a loop with a typed question prop", () => {
    const questions: QuestionDefinition[] = [
      makeQuestion({
        id: "11111111-1111-1111-1111-111111111111",
        key: "first_name",
        label: "First name",
        required: true,
        defaultValue: "Ada",
      }),
      makeQuestion({
        id: "22222222-2222-2222-2222-222222222222",
        key: "last_name",
        label: "Last name",
        defaultValue: "Lovelace",
      }),
    ];

    render(
      <div>
        {questions.map((question) => (
          <QuestionRenderer key={`${question.id}-${question.key}`} question={question} />
        ))}
      </div>
    );

    const firstNameInput = screen.getByLabelText(/first name/i) as HTMLInputElement;
    const lastNameInput = screen.getByLabelText(/last name/i) as HTMLInputElement;

    expect(firstNameInput).toBeInTheDocument();
    expect(lastNameInput).toBeInTheDocument();
    expect(firstNameInput.value).toBe("Ada");
    expect(lastNameInput.value).toBe("Lovelace");
  });

  test("does not share state between instances", () => {
    const questions: QuestionDefinition[] = [
      makeQuestion({
        id: "33333333-3333-3333-3333-333333333333",
        key: "city",
        label: "City",
        defaultValue: "Raleigh",
      }),
      makeQuestion({
        id: "44444444-4444-4444-4444-444444444444",
        key: "country",
        label: "Country",
        defaultValue: "USA",
      }),
    ];

    render(
      <div>
        {questions.map((question) => (
          <QuestionRenderer key={`${question.id}-${question.key}`} question={question} />
        ))}
      </div>
    );

    const cityInput = screen.getByLabelText("City") as HTMLInputElement;
    const countryInput = screen.getByLabelText("Country") as HTMLInputElement;

    fireEvent.change(cityInput, { target: { value: "Greenville" } });

    expect(cityInput.value).toBe("Greenville");
    expect(countryInput.value).toBe("USA");
  });

  test("renders duplicate question definitions as independent instances", () => {
    const repeatedQuestion = makeQuestion({
      id: "66666666-6666-6666-6666-666666666666",
      key: "nickname",
      label: "Nickname",
      defaultValue: "Ada",
    });

    render(
      <div>
        {[0, 1].map((index) => (
          <QuestionRenderer key={`${repeatedQuestion.id}-${index}`} question={repeatedQuestion} />
        ))}
      </div>
    );

    const nicknameInputs = screen.getAllByLabelText("Nickname") as HTMLInputElement[];
    expect(nicknameInputs).toHaveLength(2);
    expect(nicknameInputs[0].id).not.toBe(nicknameInputs[1].id);

    fireEvent.change(nicknameInputs[0], { target: { value: "Countess" } });
    expect(nicknameInputs[0].value).toBe("Countess");
    expect(nicknameInputs[1].value).toBe("Ada");
  });

  test("renders without React warnings or errors", () => {
    const consoleErrorSpy = jest.spyOn(console, "error").mockImplementation(() => undefined);
    const consoleWarnSpy = jest.spyOn(console, "warn").mockImplementation(() => undefined);

    render(
      <QuestionRenderer
        question={makeQuestion({
          id: "55555555-5555-5555-5555-555555555555",
          key: "email",
          label: "Email",
          defaultValue: "",
          helpText: "We will never share your email.",
        })}
      />
    );

    expect(consoleErrorSpy).not.toHaveBeenCalled();
    expect(consoleWarnSpy).not.toHaveBeenCalled();
    consoleErrorSpy.mockRestore();
    consoleWarnSpy.mockRestore();
  });
});
