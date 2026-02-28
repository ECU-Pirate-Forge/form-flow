import React, { useEffect, useState } from "react";
import sample from "./sample-question.json";
import { QuestionRenderer } from "./components/QuestionRenderer";
import { QuestionDefinition } from "./types/QuestionDefinition";

function App() {
  const [question, setQuestion] = useState<QuestionDefinition | null>(null);

  useEffect(() => {
    setQuestion(sample);
  }, []);

  return (
    <div>
      <h1>React Dynamic Question</h1>
      {question && <QuestionRenderer question={question} />}
    </div>
  );
}

export default App;