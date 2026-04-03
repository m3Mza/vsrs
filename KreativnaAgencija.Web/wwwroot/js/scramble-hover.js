/**
 * Scramble Hover Effect
 * Text scrambles on hover for interactive elements
 */

class ScrambleHover {
  constructor(element, options = {}) {
    this.element = element;
    this.originalText = element.textContent;
    
    // Options
    this.scrambleSpeed = options.scrambleSpeed || 50;
    this.maxIterations = options.maxIterations || 8;
    this.useOriginalCharsOnly = options.useOriginalCharsOnly !== false;
    this.characters = options.characters || 'abcdefghijklmnopqrstuvwxyz!@#$%^&*()_+-=[]{}|;\':",./<>?';
    
    this.isScrambling = false;
    this.iteration = 0;
    
    this.init();
  }
  
  init() {
    this.element.addEventListener('mouseenter', () => this.startScramble());
    this.element.addEventListener('mouseleave', () => this.stopScramble());
  }
  
  startScramble() {
    if (this.isScrambling) return;
    
    this.isScrambling = true;
    this.iteration = 0;
    this.scramble();
  }
  
  stopScramble() {
    this.isScrambling = false;
    this.element.textContent = this.originalText;
  }
  
  scramble() {
    if (!this.isScrambling || this.iteration >= this.maxIterations) {
      this.element.textContent = this.originalText;
      this.isScrambling = false;
      return;
    }
    
    const scrambled = this.originalText
      .split('')
      .map((char, index) => {
        if (char === ' ') return ' ';
        
        // Gradually reveal the original text
        if (index < this.iteration) {
          return this.originalText[index];
        }
        
        return this.getRandomChar(char);
      })
      .join('');
    
    this.element.textContent = scrambled;
    this.iteration++;
    
    setTimeout(() => this.scramble(), this.scrambleSpeed);
  }
  
  getRandomChar(originalChar) {
    if (this.useOriginalCharsOnly) {
      // Use only characters from the original text
      const chars = this.originalText.replace(/\s/g, '');
      return chars[Math.floor(Math.random() * chars.length)];
    } else {
      // Use custom character set
      return this.characters[Math.floor(Math.random() * this.characters.length)];
    }
  }
}

// Initialize scramble effect on page load
document.addEventListener('DOMContentLoaded', function() {
  // Apply to nav links only
  document.querySelectorAll('.nav-link-minimal').forEach(link => {
    new ScrambleHover(link, {
      scrambleSpeed: 30,
      maxIterations: 6,
      useOriginalCharsOnly: true
    });
  });
  
  // Apply to "Detalji" links in project pregled only
  document.querySelectorAll('.portfolio-row .btn-text').forEach(link => {
    if (link.textContent.includes('Detalji')) {
      new ScrambleHover(link, {
        scrambleSpeed: 30,
        maxIterations: 6,
        useOriginalCharsOnly: true
      });
    }
  });
});
