const boxContainer = document.getElementById("boxContainer");
const horizontalButton = document.getElementById("horizontalButton");
const verticalButton = document.getElementById("verticalButton");

horizontalButton.addEventListener("click", function () {
  boxContainer.classList.remove("vertical");
  boxContainer.classList.add("horizontal");
});

verticalButton.addEventListener("click", function () {
  boxContainer.classList.remove("horizontal");
  boxContainer.classList.add("vertical");
});