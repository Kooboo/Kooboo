import request from "@/utils/request";
import type { Header } from "./types";

export const getHeader = (): Promise<Header> =>
  request.get<Header>("/bar/header");
