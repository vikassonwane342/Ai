import { GoogleGenerativeAI } from "@google/generative-ai";
import fs from "fs";
import { execSync } from "child_process";

const genAI = new GoogleGenerativeAI(process.env.GEMINI_API_KEY);
const model = genAI.getGenerativeModel({  model: "models/gemini-2.5-flash" });
const issueTitle = process.env.ISSUE_TITLE;
const issueBody = process.env.ISSUE_BODY || "No description provided";
const issueNumber = process.env.ISSUE_NUMBER;

console.log("🚀 GEMINI AI STARTED");
console.log("Issue:", issueTitle);

function readFileSafe(path) {
  try {
    return fs.readFileSync(path, "utf8");
  } catch {
    return "";
  }
}

const architecture = readFileSafe("docs/architecture.md");
const conventions = readFileSafe("docs/conventions.md");
const project = readFileSafe("docs/project.md");
const skill = readFileSafe("skills/bug-fixer.md");

const prompt = `
You are a senior .NET developer.

Fix this issue:

Title:
${issueTitle}

Description:
${issueBody}

Project Context:
${project}

Architecture:
${architecture}

Conventions:
${conventions}

Skill:
${skill}

Tasks:
1. Identify root cause
2. Fix the issue
3. Follow clean architecture

IMPORTANT:
- Return ONLY changed files
- Include FULL file path
- Provide COMPLETE code

Format:

FILE: path/to/file.cs
CODE:
<full code>
`;

const result = await model.generateContent(prompt);
const output = result.response.text();

console.log("AI OUTPUT:\n", output);

fs.writeFileSync("ai-fix.txt", output);

// Git operations
execSync(`git checkout -b bugfix-${issueNumber}`);

// TEMP demo change
fs.appendFileSync("README.md", "\nAI Fix applied\n");

execSync("git add .");
execSync(`git commit -m "AI Fix: ${issueTitle}"`);
execSync(`git push origin bugfix-${issueNumber}`);

console.log("✅ DONE");
