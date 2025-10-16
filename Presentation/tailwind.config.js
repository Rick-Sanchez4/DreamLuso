/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: 'class',
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      fontFamily: {
        'poppins': ['Poppins', 'sans-serif'],
        'inter': ['Inter', 'sans-serif'],
        'playfair': ['Playfair Display', 'serif']
      },
      colors: {
        primary: {
          50: '#f0fdf4',
          100: '#dcfce7',
          200: '#bbf7d0',
          300: '#86efac',
          400: '#4ade80',
          500: '#10b981',  // Verde esmeralda neon
          600: '#059669',
          700: '#047857',
          800: '#065f46',
          900: '#064e3b',
          950: '#022c22',
        },
        secondary: {
          50: '#f0fdfa',
          100: '#ccfbf1',
          200: '#99f6e4',
          300: '#5eead4',
          400: '#2dd4bf',
          500: '#14b8a6',  // Teal/Turquesa neon
          600: '#0d9488',
          700: '#0f766e',
          800: '#115e59',
          900: '#134e4a',
        },
        accent: {
          50: '#fefce8',
          100: '#fef9c3',
          200: '#fef08a',
          300: '#fde047',
          400: '#facc15',
          500: '#eab308',  // Amarelo limão
          600: '#ca8a04',
          700: '#a16207',
          800: '#854d0e',
          900: '#713f12',
        },
        dark: {
          50: '#f8fafc',
          100: '#e1e8ef',
          200: '#c3d1e0',
          300: '#9fb4cc',
          400: '#7a96b5',
          500: '#5a7a9e',
          600: '#475f82',
          700: '#364766',
          800: '#1e293b',
          850: '#0f1729',
          900: '#0a0e27',
          950: '#050714',
        }
      },
      backgroundImage: {
        'gradient-radial': 'radial-gradient(var(--tw-gradient-stops))',
        'gradient-primary': 'linear-gradient(135deg, #10b981 0%, #059669 100%)',
        'gradient-luxury': 'linear-gradient(135deg, #10b981 0%, #14b8a6 50%, #2dd4bf 100%)',
        'gradient-sunset': 'linear-gradient(135deg, #eab308 0%, #facc15 50%, #fde047 100%)',
        'gradient-dark': 'linear-gradient(135deg, #0a0e27 0%, #1e293b 50%, #334155 100%)',
        'gradient-dark-green': 'linear-gradient(to bottom right, #0f172a, #1e293b, #064e3b)',
        'gradient-hero': 'linear-gradient(to bottom, rgba(0,0,0,0.4), rgba(0,0,0,0.6))',
      },
      animation: {
        'fade-in': 'fadeIn 0.6s ease-in-out',
        'fade-in-up': 'fadeInUp 0.8s ease-out',
        'scale-in': 'scaleIn 0.5s ease-out',
        'slide-in-right': 'slideInRight 0.6s ease-out',
        'bounce-slow': 'bounce 3s infinite',
      },
      keyframes: {
        fadeIn: {
          '0%': { opacity: '0' },
          '100%': { opacity: '1' },
        },
        fadeInUp: {
          '0%': { opacity: '0', transform: 'translateY(20px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' },
        },
        scaleIn: {
          '0%': { opacity: '0', transform: 'scale(0.95)' },
          '100%': { opacity: '1', transform: 'scale(1)' },
        },
        slideInRight: {
          '0%': { opacity: '0', transform: 'translateX(20px)' },
          '100%': { opacity: '1', transform: 'translateX(0)' },
        },
      },
      boxShadow: {
        'luxury': '0 20px 60px -12px rgba(99, 102, 241, 0.25)',
        'luxury-lg': '0 25px 80px -15px rgba(99, 102, 241, 0.35)',
        'glow': '0 0 30px rgba(99, 102, 241, 0.3)',
      },
    },
  },
  plugins: [],
}

