"use strict";

let hour = 0;
let minute = 0;
let second = 0;

let cron;

function start() {
  cron = setInterval(() => {
    timer();
  }, 1000);
}

function pause() {
  clearInterval(cron);
}

function reset() {
  clearInterval(cron);
  hour = 0;
  minute = 0;
  second = 0;

  document.getElementById("counter").innerText = "00:00:00";
}

function timer() {
  second++;
  if (second == 60) {
    second = 0;
    minute++;
  }
  if (minute == 60) {
    minute = 0;
    hour++;
  }

  let format =
    (hour < 10 ? "0" + hour : hour) +
    ":" +
    (minute < 10 ? "0" + minute : minute) +
    ":" +
    (second < 10 ? "0" + second : second);

  document.getElementById("counter").innerText = format;

  return format;
}

document.getElementById("start").addEventListener("click", start);
document.getElementById("pause").addEventListener("click", pause);
document.getElementById("reset").addEventListener("click", reset);
