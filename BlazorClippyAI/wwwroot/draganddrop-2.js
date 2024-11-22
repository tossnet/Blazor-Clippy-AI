export function makeClippyDraggable() {
    const container = document.getElementById("clippy-container");
    const handle = document.getElementById("clippy");

    if (!container || !handle) {
        console.error(`Container or handle element not found. Check IDs: "clippy-container", "clippy"`);
        return;
    }

    let isDragging = false;
    let startX = 0;
    let startY = 0;
    let startBottom = 0;
    let startRight = 0;

    // Commence à glisser lorsqu'on clique sur le handle
    handle.addEventListener('mousedown', (e) => {
        isDragging = true;
        startX = e.clientX;
        startY = e.clientY;

        // Calculer les positions initiales du conteneur
        const containerRect = container.getBoundingClientRect();
        startBottom = window.innerHeight - containerRect.bottom;
        startRight = window.innerWidth - containerRect.right;

        container.style.cursor = 'grabbing';
        e.preventDefault(); // Empêche la sélection de texte pendant le glissement
    });

    document.addEventListener('mousemove', (e) => {
        if (isDragging) {
            // Calculer le déplacement
            const deltaX = e.clientX - startX;
            const deltaY = e.clientY - startY;

            // Mettre à jour `bottom` et `right` dynamiquement
            container.style.bottom = `${startBottom - deltaY}px`;
            container.style.right = `${startRight - deltaX}px`;
        }
    });

    document.addEventListener('mouseup', () => {
        isDragging = false;
        container.style.cursor = 'grab';
    });
}
