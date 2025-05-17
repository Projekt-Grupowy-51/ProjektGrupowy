export type ThemeType = "light" | "dark";

class ThemeService {
  private static instance: ThemeService;
  private theme: ThemeType = "dark";

  private constructor() {
    const saved = localStorage.getItem("theme") as ThemeType | null;
    this.theme = saved === "light" || saved === "dark" ? saved : "dark";
    this.applyTheme();
  }

  static getInstance(): ThemeService {
    if (!this.instance) {
      this.instance = new ThemeService();
    }
    return this.instance;
  }

  getTheme(): ThemeType {
    return this.theme;
  }

  toggleTheme(): void {
    this.theme = this.theme === "light" ? "dark" : "light";
    localStorage.setItem("theme", this.theme);
    this.applyTheme();
  }

  private applyTheme(): void {
    document.documentElement.setAttribute("data-bs-theme", this.theme);
  }
}

export default ThemeService.getInstance();
