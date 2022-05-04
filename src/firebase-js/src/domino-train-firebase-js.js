// Module Manager for registering the modules of the chart
import { ModuleManager } from 'igniteui-webcomponents-core';
// Bullet Graph Module
import { IgcRadialGaugeCoreModule } from 'igniteui-webcomponents-gauges';
import { IgcRadialGaugeModule } from 'igniteui-webcomponents-gauges';

import { firebase, initializeApp } from 'firebase/app';
import { getAuth, onAuthStateChanged, createUserWithEmailAndPassword } from 'firebase/auth';
import { getDatabase } from "firebase/database";
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

window.firebaseApiKey = function () {
    var keyElement = document.getElementById("firebaseApiKey");
    if (keyElement === null || keyElement === undefined) {
        return null;
    }
    return keyElement.innerText;
}

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
    apiKey: window.firebaseApiKey(),
    authDomain: "domino-train.firebaseapp.com",
    projectId: "domino-train",
    storageBucket: "domino-train.appspot.com",
    messagingSenderId: "234371473873",
    appId: "1:234371473873:web:82913da0d705b88d8680f0",
    measurementId: "G-FZM4C3Q0VQ"
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);

export const auth = getAuth()
const database = getDatabase();


window.firebaseInitialize = function (dotNetObjectReference) {
    // // 🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥
    // // The Firebase SDK is initialized and available here!
    //
    // firebase.auth().onAuthStateChanged(user => { });
    // firebase.database().ref('/path/to/ref').on('value', snapshot => { });
    // firebase.firestore().doc('/foo/bar').get().then(() => { });
    // firebase.functions().httpsCallable('yourFunction')().then(() => { });
    // firebase.messaging().requestPermission().then(() => { });
    // firebase.storage().ref('/path/to/ref').getDownloadURL().then(() => { });
    // firebase.analytics(); // call to activate
    // firebase.analytics().logEvent('tutorial_completed');
    // firebase.performance(); // call to activate
    //
    // // 🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥

    try {
        console.log(`Firebase app "${app.name}" loaded`);
    } catch (e) {
        console.error(e);
    }
    onAuthStateChanged(auth, user => {
        dotNetObjectReference.invokeMethodAsync('OnAuthStateChanged', user);
    });
};

window.firebaseCreateUser = async function (user, password) {
    var userJsonData = null;
    await createUserWithEmailAndPassword(auth, user, password).then((userCredential) => {
        userJsonData = JSON.stringify(userCredential.user);
    }).catch((e) => {
        console.error(e)
    });
    console.log(userJsonData);
    return userJsonData;
}

export default { app, auth, database }