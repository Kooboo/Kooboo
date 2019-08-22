const testsContext = (require as any).context(".", true, /\.test$/);
testsContext.keys().forEach(testsContext);
