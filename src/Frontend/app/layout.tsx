import type { Metadata } from "next";
import { Providers } from "./providers";
import "./globals.css";

// SEO cơ bản (NFR-SEO). Từng trang sẽ tự ghi đè title qua metadata riêng.
export const metadata: Metadata = {
  title: { template: "%s | Culinary Blog", default: "Culinary Blog" },
  description: "Khám phá, chia sẻ và quản lý công thức nấu ăn",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="vi">
      <body>
        <Providers>{children}</Providers>
      </body>
    </html>
  );
}
