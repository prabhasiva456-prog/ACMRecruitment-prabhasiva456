const technologies = [
  "HTML",
  "CSS",
  "JavaScript",
  "Git",
  "GitHub"
];

const technologyList = document.getElementById("technologyList");

technologies.forEach(function (technology) {
  const listItem = document.createElement("li");
  listItem.textContent = technology;
  technologyList.appendChild(listItem);
});