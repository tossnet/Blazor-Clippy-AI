export function initializeClippy() {
    const clippy = document.getElementById("clippy");

    const images = [
        "assets/clippy.png",
        "assets/clippy2.png",   // Sourcils haut
        "assets/clippy3.png"    //Mouv yeux
    ];

    let currentImageIndex = 0; 
    let animationInterval;

    // Fonction pour alterner les images
    function changeClippyImage(index) {
        clippy.src = images[index]; 
    }

    function startAnimation() {
        clearInterval(animationInterval); //  seul intervalle actif?
        const delay = Math.random() * 1000 + 2000; // entre 2 et 4 secondes

        animationInterval = setInterval(() => {
            // Passe à l'image suivante dans la liste
            currentImageIndex = (currentImageIndex + 1) % images.length;

            changeClippyImage(currentImageIndex);

            if (currentImageIndex === 1) {
                setTimeout(() => {
                    changeClippyImage(0); 
                }, 200); // Délai court pour l'image des sourcils
            }
            else {
                // Pause plus longue pour les autres images
                clearInterval(animationInterval);
                setTimeout(() => {
                    startAnimation();
                }, 1000); // Temps prolongé (1 seconde)
            }
        }, delay); 
    }

    // Interaction : clic sur Clippy 
    clippy.addEventListener("click", () => {
        clearInterval(animationInterval); 
        changeClippyImage(2);
        setTimeout(() => {
            changeClippyImage(0); 
            startAnimation(); 
        }, 2000);
    });

    startAnimation(); // Démarre l'animation au chargement
}