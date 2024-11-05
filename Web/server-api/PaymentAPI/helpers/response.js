export function Send(res, status, content) {
    res.status(status);
    res.json(content);
};