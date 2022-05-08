// Module Manager for registering the modules of the chart
import { ModuleManager } from 'igniteui-webcomponents-core';
// Bullet Graph Module
import { IgcRadialGaugeCoreModule } from 'igniteui-webcomponents-gauges';
import { IgcRadialGaugeModule } from 'igniteui-webcomponents-gauges';

import { initializeApp } from "firebase/app";
import { getAnalytics } from "firebase/analytics";
// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries


// register the modules
ModuleManager.register(
    IgcRadialGaugeCoreModule,
    IgcRadialGaugeModule
);

window.updateValue = function (value) {
    var rg = document.getElementById("rg");
    rg.value = value;
}


// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
    apiKey: "AIzaSyD1B7iTPwBAa4RsZAt5onQIdFZSNrBompM",
    authDomain: "domino-train.firebaseapp.com",
    projectId: "domino-train",
    storageBucket: "domino-train.appspot.com",
    messagingSenderId: "234371473873",
    appId: "1:234371473873:web:82913da0d705b88d8680f0",
    measurementId: "G-FZM4C3Q0VQ"
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);
const analytics = getAnalytics(app);