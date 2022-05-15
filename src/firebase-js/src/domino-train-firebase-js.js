// Module Manager for registering the modules of the chart
import { ModuleManager } from 'igniteui-webcomponents-core';
// Bullet Graph Module
import { IgcRadialGaugeCoreModule } from 'igniteui-webcomponents-gauges';
import { IgcRadialGaugeModule } from 'igniteui-webcomponents-gauges';

import { firebase, initializeApp } from 'firebase/app';
import {
    getAuth, onAuthStateChanged, signOut, updateProfile,
    createUserWithEmailAndPassword, signInWithEmailAndPassword,
    sendEmailVerification, sendPasswordResetEmail, 
    signInWithPopup, GoogleAuthProvider
} from 'firebase/auth';
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

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
/*
 * https://firebase.google.com/docs/projects/api-keys?msclkid=50c2da1bd15411ec864a2051a4985260
 * API keys for Firebase are different from typical API keys
 * Unlike how API keys are typically used, API keys for Firebase services are not used to control access to backend resources; that can only be done with Firebase Security Rules (to control which users can access resources) and App Check (to control which apps can access resources).
 *
 * Usually, you need to fastidiously guard API keys (for example, by using a vault service or setting the keys as environment variables); however, API keys for Firebase services are ok to include in code or checked-in config files.
 *
 * Although API keys for Firebase services are safe to include in code, there are a few specific cases when you should enforce limits for your API key; for example, if you're using Firebase ML, Firebase Authentication with the email/password sign-in method, or a billable Google Cloud API. Learn more about these cases later on this page.
 */
const firebaseConfig = {
    apiKey: atob('QUl6YVN5RF91X1pLckJob1hzdWhyVVdKZEFKaUxZdDVBQkY3bXE0'),
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
const provider = new GoogleAuthProvider();
window.firebaseDotNetFirebaseAuthReference = null;

window.firebaseInitialize = async function (dotNetObjectReference) {
    if (window.firebaseDotNetFirebaseAuthReference !== null) {
        console.log("Firebase already initialized, skipping");
        return;
    }
    window.firebaseDotNetFirebaseAuthReference = dotNetObjectReference;
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
    // add observer to auth state changed
    onAuthStateChanged(auth, window.firebaseAuthStateChanged);
};

window.firebaseIsInitialized = function () {
    if ((window.firebaseDotNetFirebaseAuthReference === undefined) ||
        (window.firebaseDotNetFirebaseAuthReference === null)) {
        console.error("firebase is not initialized");
        return false;
    }
    return true;
};

window.firebaseAuthStateChanged = async function (user) {
    if (!window.firebaseIsInitialized()) {
        return;
    }
    console.log(user);
    await window.firebaseDotNetFirebaseAuthReference.invokeMethodAsync('OnAuthStateChanged', user);
};

window.firebaseUpdateProfile = async function (userData) {
    if (!window.firebaseIsInitialized()) {
        return;
    }
    var updated = null;
    await updateProfile(auth.currentUser, userData).then(async () => {
        // Profile updated!
        // ...
        updated = true;
    }).catch((error) => {
        console.error(error);
        // An error occurred
        // ...
        updated = false;
    });
    return updated;
};

window.firebaseSendEmailVerification = async function () {
    if (!window.firebaseIsInitialized()) {
        return;
    }
    var sent = null;
    await sendEmailVerification(auth.currentUser)
        .then(() => {
            // Email verification sent!
            // ...
            sent = true;
        }).catch((error) => {
            sent = false;
        });
    return sent;
}

window.firebaseSendEmailPasswordReset = async function () {
    if (!window.firebaseIsInitialized()) {
        return;
    }
    var reset = null;
    await sendPasswordResetEmail(auth, email).then(async () => {
        reset = true;
    }).catch((error) => {
        reset = false;
    });
    return reset;
}

window.firebaseCreateUser = async function (email, password) {
    if (!window.firebaseIsInitialized()) {
        return;
    }
    var userJsonData = null;
    await createUserWithEmailAndPassword(auth, email, password).then(async (userCredential) => {
        userJsonData = JSON.stringify(userCredential.user);
        await window.firebaseAuthStateChanged(userCredential.user);
    }).catch((e) => {
        console.error(e)
    });
    console.log(userJsonData);
    return userJsonData;
};

window.firebaseLoginUser = async function (email, password) {
    if (!window.firebaseIsInitialized()) {
        return;
    }
    var userJsonData = null;
    await signInWithEmailAndPassword(auth, email, password)
        .then(async (userCredential) => {
            // Signed in 
            userJsonData = JSON.stringify(userCredential.user);
            await window.firebaseAuthStateChanged(userCredential.user);
        })
        .catch(async (error) => {
            const errorCode = error.code;
            const errorMessage = error.message;
            console.error(error)
            await window.firebaseAuthStateChanged(null);
            
        });
    console.log(userJsonData);
    return userJsonData;
};

window.firebaseLoginWithGooglePopup = async function () {
    if (!window.firebaseIsInitialized()) {
        return;
    }
    var userJsonData = null;    
    signInWithPopup(auth, provider)
        .then(async (result) => {
            // This gives you a Google Access Token. You can use it to access the Google API.
            const credential = GoogleAuthProvider.credentialFromResult(result);
            const token = credential.accessToken;
            // The signed-in user info.
            const user = result.user;
            userJsonData = JSON.stringify(user);
            await window.firebaseAuthStateChanged(result.user);
        }).catch(async (error) => {
            // Handle Errors here.
            const errorCode = error.code;
            const errorMessage = error.message;
            // The email of the user's account used.
            const email = error.email;
            // The AuthCredential type that was used.
            const credential = GoogleAuthProvider.credentialFromError(error);
            console.log(error);
            await window.firebaseAuthStateChanged(null);
        });
    console.log(userJsonData);
    return userJsonData;
};

window.firebaseSignOut = async function () {
    if (!window.firebaseIsInitialized()) {
        return;
    }
    await signOut(auth);
}

export default { app, auth, database }