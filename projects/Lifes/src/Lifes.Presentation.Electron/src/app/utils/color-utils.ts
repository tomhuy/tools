export const STANDARD_COLOR_PALETTE = [
  "#FFFFFF", "#F2F2F2", "#D9D9D9", "#D9E1F2", "#FCE4D6", "#FDE9D9",
  "#FFF2CC", "#D9EAD3", "#E2EFDA", "#D9EBF7", "#DAE8FC", "#E1D5E7",
  "#BFBFBF", "#A5A5A5", "#7B7B7B", "#4472C4", "#ED7D31", "#FF0000",
  "#FFC000", "#70AD47", "#5B9BD5", "#255E91", "#44546A", "#262626",
  "#4CAF50", "#2196F3", "#F44336", "#9C27B0", "#FF9800", "#009688", "#FFC107", "#3F51B5",
  "#607D8B", "#00BCD4", "#E91E63",

  "#1D9E75", "#E1F5EE", "#0F6E56",
  "#7F77DD", "#EEEDFE", "#534AB7",
  "#EF9F27", "#FAEEDA", "#854F0B",
  "#D4537E", "#FBEAF0", "#993556",
  "#378ADD", "#E6F1FB", "#185FA5"

];

const LIGHT_COLORS = new Set([
  "#FFFFFF", "#F2F2F2", "#D9D9D9", "#D9E1F2", "#FCE4D6", "#FDE9D9",
  "#FFF2CC", "#D9EAD3", "#E2EFDA", "#D9EBF7", "#DAE8FC", "#E1D5E7"
]);

export function getSolidBgColor(category: string | null | undefined): string {
  if (!category) return "#9E9E9E";

  if (category.startsWith("#")) return category;

  switch (category) {
    case "Work": return "#4CAF50";
    case "Personal": return "#2196F3";
    case "Health": return "#F44336";
    case "Learning": return "#9C27B0";
    case "Travel": return "#FF9800";
    case "Event": return "#009688";
    case "Review": return "#FFC107";
    case "Planning": return "#3F51B5";
    case "Conference": return "#607D8B";
    case "Competition": return "#00BCD4";
    case "Release": return "#E91E63";
    case "Psychology": return "#4CAF50";
    default: return "#9E9E9E";
  }
}

export function getContrastColor(hexColor: string): string {
  return LIGHT_COLORS.has(hexColor.toUpperCase()) ? "#2D3748" : "#FFFFFF";
}

export function getSolidFgColor(color: string | null | undefined): string {
  if (!color) return "#FFFFFF";

  if (color.startsWith("#")) {
    return getContrastColor(color);
  }

  return color === "Review" ? "#000000" : "#FFFFFF";
}
