import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  images: {
    // Ảnh công thức lưu trên MinIO (dev: localhost:9000).
    remotePatterns: [
      { protocol: "http", hostname: "localhost", port: "9000", pathname: "/**" },
      { protocol: "https", hostname: "lh3.googleusercontent.com" }, // avatar Google
    ],
    formats: ["image/avif", "image/webp"],
  },
};

export default nextConfig;
