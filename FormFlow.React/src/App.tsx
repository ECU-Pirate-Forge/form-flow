import React, { useEffect, useState } from "react";
import sample from "./multiple-sample-questions.json";
import { QuestionRenderer } from "./components/QuestionRenderer";
import { QuestionDefinition } from "./types/QuestionDefinition";

function App() {
  const [questions, setQuestions] = useState<QuestionDefinition[]>([]);

  useEffect(() => {
    setQuestions(sample);
  }, []);

  return (
    <div>
      <h1>React Dynamic Question</h1>
      <p>Loaded {questions.length} questions.</p>

      {questions.map((q) => (
        <QuestionRenderer key={`${q.id}-${q.key}`} question={q} />
      ))}
    </div>
  );
}

export default App;