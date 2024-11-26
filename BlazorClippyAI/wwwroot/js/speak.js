
/**
 * Parle le texte fourni avec la langue et la voix spécifiée.
 * @param {string} textToSpeech - Le texte à lire.
 * @param {string} [lang="fr-FR"] - Langue utilisée pour la synthèse vocale.
 */
export const speak = (textToSpeech, lang = "fr-FR") => {
    const synth = window.speechSynthesis;

    // Vérifier si la synthèse est déjà en cours
    if (synth.speaking) {
        console.error("speechSynthesis.speaking");
        return;
    }

    if (textToSpeech) {
        const utterThis = new SpeechSynthesisUtterance(textToSpeech);

        // Événements liés à l'utterance
        utterThis.onend = () => console.log("SpeechSynthesisUtterance.onend");
        utterThis.onerror = (event) => console.error("SpeechSynthesisUtterance.onerror", event);

        // Chercher les voix disponibles
        const voices = synth.getVoices();
        const targetVoiceName = "Microsoft Eloise Online (Natural) - French (France)";

        // Trouver la voix cible ou une voix par défaut pour la langue
        const finalVoice =
            voices.find(voice => voice.name === targetVoiceName) ||
            voices.find(voice => voice.lang === lang);

        // Si une voix est trouvée, la configurer
        if (finalVoice) {
            utterThis.voice = finalVoice;
            utterThis.pitch = 1; // 0.1 à 10
            utterThis.rate = 1;  // 0.5 à 2

            // Lancer la synthèse
            synth.speak(utterThis);
        } else {
            console.error(`Aucune voix trouvée pour "${targetVoiceName}" ou "${lang}".`);
        }
    }
};

export default speak;
